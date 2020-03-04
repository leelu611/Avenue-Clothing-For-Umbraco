﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using IImageService = UCommerce.Content.IImageService;

namespace UCommerce.RazorStore.Controllers
{
    public class SearchController : RenderMvcController
    {
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();
        public CatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<CatalogLibrary>();

        // GET: Search
        public ActionResult Index(ContentModel model)
        {
            var keyword = System.Web.HttpContext.Current.Request.QueryString["search"];
            IEnumerable<Product> products = new List<Product>();
            ProductsViewModel productsViewModel = new ProductsViewModel();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                products = Product.Find(p =>
                    p.VariantSku == null
                    && p.DisplayOnSite
                    &&
                    (
                        p.Sku.Contains(keyword)
                        || p.Name.Contains(keyword)
                        || p.ProductDescriptions.Any(d => d.DisplayName.Contains(keyword)
                                                          || d.ShortDescription.Contains(keyword)
                                                          || d.LongDescription.Contains(keyword)
                        )
                    )
                );
            }

            foreach (var product in products.Where(x => x.DisplayOnSite))
            {
                productsViewModel.Products.Add(new ProductViewModel()
                {
                    Url = UrlService.GetUrl(product),
                    Name = product.DisplayName(),
                    Sku = product.Sku,
                    IsVariant = product.IsVariant,
                    LongDescription = product.LongDescription(),
                    PriceCalculation = CatalogLibrary.CalculatePrices(product),
                    ThumbnailImageUrl = ObjectFactory.Instance.Resolve<IImageService>()
                        .GetImage(product.ThumbnailImageMediaId).Url,
                    VariantSku = product.VariantSku
                });
            }

            return View("/Views/Search.cshtml", productsViewModel);
        }
    }
}