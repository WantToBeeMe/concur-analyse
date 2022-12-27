﻿using System;
using Quiz;

// todo: Complete the implementation

/// Concurrent version of the Quiz
namespace ConcQuiz
{
    public class ConcAnswer: Answer
    {
        public ConcAnswer(ConcStudent std, string txt = ""): base(std,txt){}
    }
    public class ConcQuestion : Question
    {
        //todo: add required fields, if necessary

        public ConcQuestion(string txt, string tcode) : base(txt, tcode){}

        public override void AddAnswer(Answer a)
        {
            //todo: implement the body 
        }
    }

    public class ConcStudent: Student
    {
        // todo: add required fields

        public ConcStudent(int num, string name): base(num,name){}

        public override void AssignExam(Exam e)
        {
            //todo: implement the body
        }

        public override void StartExam()
        {
            //todo: implement the body
        }

        public override void Think()
        {
            //todo: implement the body
            base.Think();
        }

        public override void ProposeAnswer()
        {
            //todo: implement the body
        }

        public override void Log(string logText = "")
        {
            base.Log();
        }

    }
    public class ConcTeacher: Teacher
    {
        //todo: add required fields, if necessary

        public ConcTeacher(string code, string name) : base(code,name){}

        public override void AssignExam(Exam e)
        {
            //todo: implement the body
            base.AssignExam(e);
        }
        public override void Think()
        {
            //todo: implement the body
            base.Think();
        }
        public override void ProposeQuestion()
        {
            //todo: implement the body
            base.ProposeQuestion();
        }
        public override void PrepareExam(int maxNumOfQuestions)
        {
            //todo: implement the body
            base.PrepareExam(maxNumOfQuestions);
        }
        public override void Log(string logText = "")
        {
            base.Log();
        }
    }
    public class ConcExam: Exam
    {
        //todo: add required fields, if necessary
        //TBC niet final
        Mutex mutex;

        public ConcExam(int number, string name = "") : base(number,name){
            mutex = new Mutex();
        }

        public override void AddQuestion(Teacher teacher, string text)
        {
            //todo: implement the body
            //niet final TBC
            lock(mutex) {
                base.AddQuestion(teacher, text);
            }
            System.Console.WriteLine("this is concurrent exam addquestion");
        }
        public override void Log(string logText = "")
        {   
            base.Log();
        }
    }

    public class ConcClassroom : Classroom
    {
        //todo: add required fields, if necessary
        //niet final TBC 
        public ConcExam ConcExam;

        public ConcClassroom(int examNumber = 1, string examName = "Programming") : base(examNumber, examName)
        {
            //todo: implement the body
            this.ConcExam = new ConcExam(examNumber, examName); // only one exam
        }

        public override void SetUp()
        {
            //todo: implement the body
            for(int i = 0; i<FixedParams.maxNumOfStudents; i++)
			{
				string std_name = " STUDENT NAME"; //todo: to be generated later
				this.Students.AddLast(new ConcStudent(i + 1, std_name));
			}
			for(int i=0; i<FixedParams.maxNumOfTeachers; i++)
            {
                string teacher_name = " TEACHER NAME"; //todo: to be generated later
                this.Teachers.AddLast(new ConcTeacher((i + 1).ToString(), teacher_name));
			}
			// assign exams
			foreach (ConcTeacher t in this.Teachers)
				t.AssignExam(this.ConcExam);
        }

        public override void PrepareExam(int maxNumOfQuestion)
        {
            //todo: implement the body
            //zoiets idk
            //TBC niet final
            List<Thread> threads = new List<Thread>();
            foreach (ConcTeacher t in this.Teachers) {
                Thread tr = new Thread (() => t.PrepareExam(maxNumOfQuestion));
                threads.Add(tr);
                tr.Start();
            }
            foreach(Thread thr in threads) {
                thr.Join();
            }
        }
        public override void DistributeExam()
        {
            //todo: implement the body
        }
        public override void StartExams()
        {
            //todo: implement the body
        }

        public string GetStatistics()
        {
            string result = "" , nl = "\n";
            int totalNumOfAnswers = 0;
            foreach (Question q in this.Exam.Questions)
                totalNumOfAnswers += q.Answers.Count;
            result = "#Students: " + this.Students.Count.ToString() + nl +
                "#Teachers: " + this.Teachers.Count.ToString() + nl +
                "#Questions: " + this.Exam.Questions.Count.ToString() + nl +
                "#Answers: " + totalNumOfAnswers.ToString();
            return result;
        }
    }
    //THIS CLASS (QUIZCONCURRENT) SHOULD NOT BE CHANGED
    public class QuizConcurrent
    {
        ConcClassroom classroom;

        public QuizConcurrent()
        {
            this.classroom = new ConcClassroom();
        }
        public void RunExams()
        {
            classroom.SetUp();
            classroom.PrepareExam(Quiz.FixedParams.maxNumOfQuestions);
            //classroom.DistributeExam();
            //classroom.StartExams();
        }
        public string FinalResult()
        {
            return classroom.GetStatistics();
        }

    }
}

