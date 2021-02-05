1. An Empty API project of type Asp .Net Core Web Application is opened.
2. The Models folder opens.
3. Models => Category.cs opens.

    general numbering Status {Active = 1, Modified = 2, Passive = 3}

    public class Category
    {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id {get; Set; }

        [Required (ErrorMessage = "Must write a title")]
        [MinLength (2, ErrorMessage = "Minimum length is 2")]
        [RegularExpression (@ "^ [a-zA-Z] + $", ErrorMessage = "Only allowed letters")]
        public string Name {get; Set; }

        [Is necessary]
        public string Slug {get; Set; }

        custom DateTime _createDate = DateTime.Now;
        generic DateTime CreateDate {get => _createDate; set => _createDate = value; }

        public DateTime? Update Date {get; Set; }
        Public DateTime? DeleteDate {get; Set; }

        Exception _status = Status.Active;
        general Status Status {get => _status; set => _status = value; }
    }

4. Models => Context folder opens => ApplicationDbContext.cs opens.
    4.1. The Microsoft.EntityFrameworkCore.SqlServer package is installed.
    public class ApplicationDbContext: DbContext
    {
        generic ApplicationDbContext (DbContextOptions <ApplicationDbContext> options): base (options) {}

        public DbSet <Category> Categories {get; Set; }
    }

    4.2. ApplicationDbContext.cs is injected into the Configure () method of the Startup.cs class.

    services.AddDbContext <ApplicationDbContext> (options => options. UsageSqlServer (Configuration.GetConnectionString ("DefaultConnection")));

    4.3. The string "DefaultConnection" is written to the appsetting.json file.

    "ConnectionStrings": {
        "DefaultConnection": "Server=NIHOO;Database=CMSDb;uid=nihatcan;pwd=123;"
    }

5. Swashbuckle Integration
   5.1. Sites you are on for Swager.UI and Swashbuckle implementations
        5.1.1. Swashbuckle.AspNetCore
        5.1.2. Swashbuckle.AspNetCore.SwaggerUI

    
   5.2. Let's add the following code block to the ConfigureServices () method in the Startup.cs class.

            services.AddSwaggerGen (options =>
            {
                options.SwaggerDoc ("CMS API", new OpenApiInfo ()
                {
                    Title = "CMS API",
                    Version = "V.1",
                    Description = "CMS API",
                    Contact = new OpenApiContact () (
                        E-mail = "burak.yilmaz@bilgeadam.com",
                        Name = "Burak Yilmaz",
                        Url = new Uri ("https://github.com/nihatcanertug")
                    },
                    License = new OpenApiLicense ()
                    {
                        Name = "MIT License",
                        Url = new Uri ("https://github.com/nihatcanertug")
                    }
                });
            });

    5.3. Add the Middleware pipeline below to automatically find us when the application startup of the Swager.UI is given.
        
            app.Swagger ();
            app.SwaggerUI (options =>
            {
                options.SwaggerEndpoint ("/ swagger / CMS API / swagger.json", "CMS API");
            });

6. Controls => CategoryController.cs opens.

    [Produces ("application / json")]
    [Route ("api / [controller]")]
    [ApiController]
    public class CategoryController: ControllerBase
    {
        private read-only ApplicationDbContext _applicationDbContext;

        public CategoryController (ApplicationDbContext applicationDbContext) => _applicationDbContext = applicationDbContext;

        /// <summary>
        ///
        /// </summary>
        /// <returns> </returns>
        [HttpGet]
        public async Task <IEnumerable <Category>> GetCategories () => _applicationDbContext.Categories.Where (x => x.Status! = Status.Passive) .OrderBy (x => x.Id) .ToListAsync ();

        /// <summary>
        ///
        /// </summary>
        /// <param name = "id"> </param>
        /// <returns> </returns>
        [HttpGet ("{id: int}", Name = "GetCategoryById")]
        generic asynchronous Task <ActionResult <Category>> GetCategoryById (int id)
        {
            Category category = await _applicationDbContext.Categories.FindAsync (id);

            if (category == null) returns NotFound ();

            Return OK (category);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name = "categorySlug"> </param>
        /// <returns> </returns>
        [HttpGet ("{categorySlug}", Name = "GetCategoryBySlug")]
        generic async Task <ActionResult <Category>> GetCategoryBySlug (string categorySlug)
        {
            Category category = _applicationDbContext.Categories.FirstOrDefaultAsync (x => x.Slug == categorySlug);

            if (category == null) returns NotFound ();
             return Ok(category);
        }
        
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            _applicationDbContext.Categories.Add(category);
            await _applicationDbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategories), category);
        }

        [HttpPut("{id}", Name = "PutCategory")]
        public async Task<ActionResult<Category>> PutCategory(int id, Category category)
        {
            if (id != category.Id) return BadRequest();

            _applicationDbContext.Entry(category).State = EntityState.Modified;

            await _applicationDbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategories), category);
        }

        [HttpDelete("{id}", Name = "DeleteCategory")]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            Category category = await _applicationDbContext.Categories.FindAsync(id);

            if (category == null) return NotFound();

            _applicationDbContext.Categories.Remove(category);
            await _applicationDbContext.SaveChangesAsync();

            return NoContent(); //=> 204 status code
        }

        7. Authentication Implemantation

     Json Web Token (Jwt)
     JSON Web Token (JWT) is an open standard (RFC 7519) that defines a compact and independent way to securely transmit information between parties as JSON objects. Since this information is digitally signed, it is verifiable and reliable. JWTs can be signed using a secret (via the HMAC algorithm) or a public / private key pair using RSA or ECDSA.

     Although JWTs can be encrypted to provide privacy between parties, in this application we will focus on signed tokens and generate them during authentication. Signed tokens verify the integrity of the claims contained, while encrypted tokens hide these requests from other parties. When tokens are signed using public / private key pairs, the signature also confirms that only the party holding the private key is the party that signed it.