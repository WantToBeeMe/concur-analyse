using System;
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
        Mutex mutex;

        public ConcQuestion(string txt, string tcode) : base(txt, tcode){
            mutex = new Mutex();
        }

        public override void AddAnswer(Answer a)
        {
            //todo: implement the body 
            lock(mutex){
                this.Answers.AddLast(a);
            }
        }
    }

    public class ConcStudent: Student
    {
        // todo: add required fields

        public ConcStudent(int num, string name): base(num,name){}

        public override void AssignExam(Exam e)
        {
            //todo: implement the body
            base.AssignExam(e);
        }

        public override void StartExam()
        {
            //todo: implement the body
            base.StartExam();
        }

        public override void Think()
        {
            //todo: implement the body
            base.Think();
        }

        public override void ProposeAnswer()
        {
            //todo: implement the body
            base.ProposeAnswer();
        }

        public override void Log(string logText = "")
        {
            base.Log();
        }

    }
    public class ConcTeacher: Teacher
    {
        //todo: add required fields, if necessary

        public ConcTeacher(string code, string name) : base(code,name){
            
        }

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
        Mutex mutex;
        int ConcNumber;
        int ConcQuestionNumber;

        public ConcExam(int number, string name = "") : base(number,name){
            mutex = new Mutex();
            ConcNumber = number;

        }

        public override void AddQuestion(Teacher teacher, string text)
        {
            //making question
            ConcQuestion q = new ConcQuestion(text, teacher.Code);
            
            lock(mutex) {
                this.ConcQuestionNumber++;
                this.Questions.AddLast(q);
            }

            this.Log("[Question is added]"+q.ToString());
        }

        public override string ToString()
        {
            string delim = " : ", nl = "\n";
            return "Exam "+delim+this.ConcNumber.ToString()+delim+" Total Num Questions: "+this.ConcQuestionNumber.ToString();
        }

        public override void Log(string logText = "")
        {   
            base.Log();
        }
    }

    public class ConcClassroom : Classroom
    {
        //todo: add required fields, if necessary

        public ConcClassroom(int examNumber = 1, string examName = "Programming") : base(examNumber, examName)
        {
            //todo: implement the body
            this.Exam = new ConcExam(examNumber, examName); 
        }

        public override void SetUp()
        {
            //todo: implement the body
            for(int i = 0; i<FixedParams.maxNumOfStudents; i++)
			{
				string std_name = "STUDENT NAME"; //may be genrated later, or not
				this.Students.AddLast(new ConcStudent(i + 1, std_name));
			}
			for(int i=0; i<FixedParams.maxNumOfTeachers; i++)
            {
                string teacher_name = "TEACHER NAME"; //may be genrated later, or not
                this.Teachers.AddLast(new ConcTeacher((i + 1).ToString(), teacher_name));
			}
			// assign exams
			foreach (ConcTeacher t in this.Teachers)
				t.AssignExam(this.Exam);
        }

        public override void PrepareExam(int maxNumOfQuestion)
        {
            //todo: implement the body
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
            foreach (ConcStudent s in this.Students)
				s.AssignExam(this.Exam);
        }
        public override void StartExams()
        {
            //todo: implement the body
            List<Thread> threads = new List<Thread>();
            foreach (ConcStudent s in this.Students) {
                Thread tr = new Thread (() => s.StartExam());
                threads.Add(tr);
                tr.Start();
            }
            foreach(Thread thr in threads) {
                thr.Join();
            }
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
            classroom.DistributeExam();
            classroom.StartExams();
        }
        public string FinalResult()
        {
            return classroom.GetStatistics();
        }

    }
}

